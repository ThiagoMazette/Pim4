﻿using System.Data.OleDb;
using System.Data;
using helpdesk2018.Model;
using System;

namespace helpdesk2018.Controller
{
    public static class ctlChamados
    {
        public static string Abrirchamado(int fk_motivo, string descricao)
        {
            Conexao conexao = new Conexao();

            conexao.abrir();

            OleDbCommand cmdAbrir = conexao.Comando(@"
                insert into tb_chamados
                (fk_idusuario, fk_idmotivo, fk_idstatus, descricao) values
                (@fk_idusuario, @fk_idmotivo, @fk_idstatus, @descricao)
            ");

            int status = 1; // pode tirar essa linha e alterar para linha 36


            OleDbParameter pmtIdUsuario = cmdAbrir.CreateParameter();
            pmtIdUsuario.Value = mdlUsuario.Logado.ID;
            pmtIdUsuario.ParameterName = "@fk_idusuario";
            pmtIdUsuario.DbType = DbType.Int16;
            cmdAbrir.Parameters.Add(pmtIdUsuario);

            OleDbParameter pmtidmotivo = cmdAbrir.CreateParameter();
            pmtidmotivo.Value = fk_motivo;
            pmtidmotivo.ParameterName = "@fk_idmotivo";
            pmtidmotivo.DbType = DbType.Int16;
            cmdAbrir.Parameters.Add(pmtidmotivo);

            OleDbParameter pmtidstatus= cmdAbrir.CreateParameter();
            // pmtidstatus.Value = 1; se fizer assim tbem resolve, nao precisa de variavel
            pmtidstatus.Value = status;
            pmtidstatus.ParameterName = "@fk_idstatus";
            pmtidstatus.DbType = DbType.Int16;
            cmdAbrir.Parameters.Add(pmtidstatus);

            OleDbParameter pmtdescricao = cmdAbrir.CreateParameter();
            pmtdescricao.Value = descricao;
            pmtdescricao.ParameterName = "@descricao";
            pmtdescricao.DbType = DbType.String;
            cmdAbrir.Parameters.Add(pmtdescricao);

            /*cmdAbrir.Parameters.AddWithValue("@fk_idusuario", mdlUsuario.Logado.ID);
            cmdAbrir.Parameters.AddWithValue("@fk_idmotivo", fk_motivo);
            cmdAbrir.Parameters.AddWithValue("@descricao", descricao);
            cmdAbrir.Parameters.AddWithValue("@fk_idstatus", status);*/

            string retorno = "";

            cmdAbrir.ExecuteNonQuery();

            OleDbCommand cmdOS = conexao.Comando(@"
            select top 1 * from tb_chamados order by OS DESC
            ");

            OleDbDataReader reader = cmdOS.ExecuteReader();
            
            while (reader.Read())
            {
                retorno = reader["OS"].ToString();

            }
            reader.Close();
            conexao.Fechar();

            return retorno;
        }

        public static void getChamado()
        {
            Conexao conexao = new Conexao();
            conexao.abrir();
            OleDbCommand cmd = conexao.Comando(@"
                select
                    tb_chamados.os,
                    tb_chamados.criado_em as Aberto,
                    tb_chamados.fechado_em as Fechado,
                    tb_usuarios.nome as nome_usuario,
                    tb_chamados.resposta as chamado_resposta,
                    tb_chamados.fk_idstatus as chamado_status,
                    tb_empresas.nome as nome_empresa,
                    tb_motivos.descricao as descricao_motivo,
                    tb_status.descricao as descricao_status,
                    tb_chamados.descricao
                from (
                    (
                        (
                            tb_chamados
                            inner join tb_usuarios
                            on tb_usuarios.idusuario = tb_chamados.fk_idusuario
                        )
                        inner join tb_empresas
                        on tb_empresas.idempresa = tb_usuarios.fk_idempresa
                    )
                    inner join tb_motivos
                    on tb_motivos.idmotivo = tb_chamados.fk_idmotivo
                )
                inner join tb_status
                on tb_status.idstatus = tb_chamados.fk_idstatus
                where tb_chamados.os = @os
            ");

            cmd.Parameters.AddWithValue("@os", mdlChamados.Chamado.OS);

            OleDbDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                mdlChamados.Chamado.Resposta = reader["chamado_resposta"].ToString();
                mdlChamados.Chamado.Status = reader["chamado_status"].ToString();
                mdlChamados.Chamado.Descricao = reader["descricao"].ToString();
                mdlChamados.Chamado.Motivo = reader["descricao_motivo"].ToString();
                mdlChamados.Chamado.Empresa = reader["nome_empresa"].ToString();
                mdlChamados.Chamado.NomeUsuario = reader["nome_usuario"].ToString();
                mdlChamados.Chamado.Aberto = reader["Aberto"].ToString();
                mdlChamados.Chamado.Fechado = reader["Fechado"].ToString();
            }
            reader.Close();
            conexao.Fechar();
        }

        public static bool FecharChamado(string resposta)
        {
            string fechado_em = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            Conexao conexao = new Conexao();
            conexao.abrir();
            string SQL = "update tb_chamados " +
                "set resposta = @resposta, " +
                "fechado_em = @fechado, " +
                "fk_idstatus = 2 " +
                "where os = @os;";

            OleDbCommand cmd = new OleDbCommand(SQL, conexao.GetConexao());
            cmd.Parameters.AddWithValue("@resposta", resposta);
            cmd.Parameters.AddWithValue("@fechado", fechado_em);
            cmd.Parameters.AddWithValue("@os", mdlChamados.Chamado.OS);

            cmd.ExecuteNonQuery();


           // SQL = "Update tb_chamados set fk_idstatus = 2 where os = @os";

            if (cmd.ExecuteNonQuery() > 0)
            {
                conexao.Fechar();
                return true;
            }else
            {
                conexao.Fechar();
                return false;
            }
        }
    }
}
