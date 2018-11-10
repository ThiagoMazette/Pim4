﻿using System.Data.OleDb;
using helpdesk2018.Model;

namespace helpdesk2018.Controller
{
    public class ctlChamados
    {
        public static string Abrirchamado(string motivo, string descricao)
        {
            Conexao conexao = new Conexao();

            conexao.abrir();

            OleDbCommand cmdAbrir = conexao.Comando(@"
                insert into tb_chamados
                (fk_idusuario, fk_idmotivo, fk_idstatus, descricao) values
                (@fk_idusuario, @fk_idmotivo, @fk_idstatus, @obs)
            ");

            cmdAbrir.Parameters.AddWithValue("@idusuario", mdlUsuario.Logado.ID);
            cmdAbrir.Parameters.AddWithValue("@nome", mdlUsuario.Logado.Nome);
            cmdAbrir.Parameters.AddWithValue("@empresa", mdlEmpresa.Logado.NomeEmpresa);
            cmdAbrir.Parameters.AddWithValue("@motivo", motivo);
            cmdAbrir.Parameters.AddWithValue("@descricao", descricao);

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
    }
}
